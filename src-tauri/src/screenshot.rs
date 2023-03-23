use std::time::{Duration, Instant};

use base64::engine::general_purpose::STANDARD as base64_engine;
use base64::Engine;
use scrap::{Display, Frame};

use crate::bitmap::BitmapEncoder;

const MAX_BLACK_FRAME_RETRIES: u8 = 3;

pub struct Screenshot {
    display: String,
    data: Vec<u8>,
}

/// Returns a vector of screen capture buffers
pub fn capture_all() -> Vec<Screenshot> {
    let displays = Display::all().unwrap();
    let mut screens = Vec::with_capacity(displays.len());

    for display in displays {
        let display_name = &display.name();

        let mut capturer = scrap::Capturer::new(display).unwrap();
        let (w, h) = (capturer.width(), capturer.height());

        let mut current_try = 0;

        // Use loop to capture the first non-error frame
        loop {
            let buffer = match capturer.frame() {
                Ok(buffer) => {
                    if buffer.iter().all(|b| b.eq(&0)) && current_try <= MAX_BLACK_FRAME_RETRIES {
                        println!("Black screenshot detected... retrying...");

                        current_try += 1;

                        continue;
                    }

                    buffer
                }
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

            screens.push(Screenshot {
                display: display_name.clone(),
                data: encode_buffer(buffer, w as u32, h as u32),
            });

            break;
        }
    }

    screens
}

/// Encodes buffer with PNG
fn encode_buffer(buffer: Frame, width: u32, height: u32) -> Vec<u8> {
    let now = Instant::now();

    let mut output = Vec::new();

    let mut encoder = BitmapEncoder::new(&mut output);

    encoder.encode_exact(&buffer, width, height).unwrap();

    println!("Encoding: {:?}", now.elapsed());

    output
}

#[derive(Clone, serde::Serialize)]
pub struct ScreenshotPayload {
    name: String,
    image: String,
}

#[derive(Clone, serde::Serialize)]
pub struct ScreenshotCapturedPayload {
    screenshots: Vec<ScreenshotPayload>,
}

pub fn get_payload(screens: Vec<Screenshot>) -> ScreenshotCapturedPayload {
    ScreenshotCapturedPayload {
        screenshots: screens
            .iter()
            .map(|screen| ScreenshotPayload {
                name: screen.display.clone(),
                image: base64_engine.encode(&screen.data),
            })
            .collect(),
    }
}