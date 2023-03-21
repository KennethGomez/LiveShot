use std::time::{Duration, Instant};

use base64::engine::general_purpose::STANDARD as base64_engine;
use base64::Engine;
use image::codecs::bmp::BmpEncoder;
use image::{ColorType, ImageEncoder};
use scrap::{Display, Frame};
use tauri::Window;

/// Returns a vector of screen capture buffers
pub fn capture_all(window: &Window) -> Vec<Vec<u8>> {
    let screen_count = window.available_monitors().unwrap().len();
    let mut screens = Vec::with_capacity(screen_count);

    for display in Display::all().unwrap() {
        let mut capturer = scrap::Capturer::new(display).unwrap();
        let (w, h) = (capturer.width(), capturer.height());

        // Use loop to capture the first non-error frame
        loop {
            let buffer = match capturer.frame() {
                Ok(buffer) => buffer,
                Err(error) => {
                    if error.kind() == std::io::ErrorKind::WouldBlock {
                        // Keep spinning.
                        std::thread::sleep(Duration::new(1, 0) / 60);

                        continue;
                    } else {
                        panic!("Error: {}", error);
                    }
                }
            };

            screens.push(encode_buffer(buffer, w, h));

            break;
        }
    }

    screens
}

/// Encodes buffer with PNG
fn encode_buffer(buffer: Frame, w: usize, h: usize) -> Vec<u8> {
    let now = Instant::now();
    // Convert from ARGB into RGB image
    let mut flipped = Vec::with_capacity(w * h * 3);
    let stride = buffer.len() / h;

    for y in 0..h {
        for x in 0..w {
            let i = stride * y + 4 * x;

            flipped.extend_from_slice(&[buffer[i + 2], buffer[i + 1], buffer[i]]);
        }
    }

    println!("Flipping: {:?}", now.elapsed());
    let now = Instant::now();

    let mut output = Vec::new();

    let encoder = BmpEncoder::new(&mut output);

    encoder
        .write_image(&flipped, w as u32, h as u32, ColorType::Rgb8)
        .unwrap();

    println!("Encoding: {:?}", now.elapsed());

    output
}

#[derive(Clone, serde::Serialize)]
pub struct ScreenshotCapturedPayload {
    images: Vec<String>,
}

pub fn get_payload(screens: Vec<Vec<u8>>) -> ScreenshotCapturedPayload {
    ScreenshotCapturedPayload {
        images: screens
            .iter()
            .map(|screen| base64_engine.encode(screen))
            .collect(),
    }
}
