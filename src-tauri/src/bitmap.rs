use byteorder::{LittleEndian, WriteBytesExt};
use std::io;
use std::io::Write;

use image::error::{EncodingError, ImageFormatHint, ParameterError, ParameterErrorKind};
use image::{ImageError, ImageFormat, ImageResult};

const BITMAPFILEHEADER_SIZE: u32 = 14;
const BITMAPV4HEADER_SIZE: u32 = 108;

pub struct BitmapEncoder<'a, W: 'a> {
    writer: &'a mut W,
}

impl<'a, W: Write + 'a> BitmapEncoder<'a, W> {
    /// Create a new encoder that writes its output to ```w```.
    pub fn new(w: &'a mut W) -> Self {
        BitmapEncoder { writer: w }
    }

    /// Encodes the image ```image```
    /// that has dimensions ```width``` and ```height```.
    /// Encodes the exact image bytes without any processing
    pub fn encode_exact(&mut self, image: &[u8], width: u32, height: u32) -> ImageResult<()> {
        // Modified version of image::BmpEncoder::encode() to preserve ARGB

        let bmp_header_size = BITMAPFILEHEADER_SIZE;

        let (dib_header_size, written_pixel_size, palette_color_count) =
            (BITMAPV4HEADER_SIZE, 4, 0);

        let row_pad_size = 0; // each row must be padded to a multiple of 4 bytes

        let image_size = width
            .checked_mul(height)
            .and_then(|v| v.checked_mul(written_pixel_size))
            .and_then(|v| v.checked_add(height * row_pad_size))
            .ok_or_else(|| {
                ImageError::Parameter(ParameterError::from_kind(
                    ParameterErrorKind::DimensionMismatch,
                ))
            })?;
        let palette_size = palette_color_count * 4; // all palette colors are BGRA
        let file_size = bmp_header_size
            .checked_add(dib_header_size)
            .and_then(|v| v.checked_add(palette_size))
            .and_then(|v| v.checked_add(image_size))
            .ok_or_else(|| {
                ImageError::Encoding(EncodingError::new(
                    ImageFormatHint::Exact(ImageFormat::Bmp),
                    "calculated BMP header size larger than 2^32",
                ))
            })?;

        // write BMP header
        self.writer.write_u8(b'B')?;
        self.writer.write_u8(b'M')?;
        self.writer.write_u32::<LittleEndian>(file_size)?; // file size
        self.writer.write_u16::<LittleEndian>(0)?; // reserved 1
        self.writer.write_u16::<LittleEndian>(0)?; // reserved 2
        self.writer
            .write_u32::<LittleEndian>(bmp_header_size + dib_header_size + palette_size)?; // image data offset

        // write DIB header
        self.writer.write_u32::<LittleEndian>(dib_header_size)?;
        self.writer.write_i32::<LittleEndian>(width as i32)?;
        self.writer.write_i32::<LittleEndian>(height as i32)?;
        self.writer.write_u16::<LittleEndian>(1)?; // color planes
        self.writer
            .write_u16::<LittleEndian>((written_pixel_size * 8) as u16)?; // bits per pixel
        if dib_header_size >= BITMAPV4HEADER_SIZE {
            // Assume ARGB32
            self.writer.write_u32::<LittleEndian>(3)?; // compression method - bitfields
        } else {
            self.writer.write_u32::<LittleEndian>(0)?; // compression method - no compression
        }
        self.writer.write_u32::<LittleEndian>(image_size)?;
        self.writer.write_i32::<LittleEndian>(0)?; // horizontal ppm
        self.writer.write_i32::<LittleEndian>(0)?; // vertical ppm
        self.writer.write_u32::<LittleEndian>(palette_color_count)?;
        self.writer.write_u32::<LittleEndian>(0)?; // all colors are important

        if dib_header_size >= BITMAPV4HEADER_SIZE {
            // Assume ARGB32
            self.writer.write_u32::<LittleEndian>(0x00FF0000)?; // red mask
            self.writer.write_u32::<LittleEndian>(0x0000FF00)?; // green mask
            self.writer.write_u32::<LittleEndian>(0x000000FF)?; // blue mask
            self.writer.write_u32::<LittleEndian>(0xFF000000)?; // alpha mask
            self.writer.write_u32::<LittleEndian>(0x73524742)?; // colorspace - sRGB

            // endpoints (3x3) and gamma (3)
            for _ in 0..12 {
                self.writer.write_u32::<LittleEndian>(0)?;
            }
        }

        // write image data
        self.encode_argb(image, width, height, row_pad_size, 4)?;

        Ok(())
    }

    fn encode_argb(
        &mut self,
        image: &[u8],
        width: u32,
        height: u32,
        row_pad_size: u32,
        bytes_per_pixel: u32,
    ) -> io::Result<()> {
        let width = width as usize;
        let height = height as usize;
        let x_stride = bytes_per_pixel as usize;
        let y_stride = width * x_stride;
        for row in (0..height).rev() {
            // from the bottom up
            let row_start = row * y_stride;

            self.writer.write(&image[row_start..][..y_stride])?;

            self.write_row_pad(row_pad_size)?;
        }

        Ok(())
    }

    fn write_row_pad(&mut self, row_pad_size: u32) -> io::Result<()> {
        for _ in 0..row_pad_size {
            self.writer.write_u8(0)?;
        }

        Ok(())
    }
}
