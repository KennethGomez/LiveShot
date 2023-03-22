#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

use tauri::State;

use crate::screenshot::ScreenshotCapturedPayload;

mod bitmap;
mod screenshot;

#[tauri::command]
fn get_screenshots(
    screenshot_state: State<ScreenshotCapturedPayload>,
) -> ScreenshotCapturedPayload {
    screenshot_state.inner().clone()
}

fn main() {
    tauri::Builder::default()
        .manage(capture_screenshots())
        .invoke_handler(tauri::generate_handler![get_screenshots])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}

fn capture_screenshots() -> ScreenshotCapturedPayload {
    let screens = screenshot::capture_all();

    println!("Found and captured {} monitor(s)", screens.len());

    screenshot::get_payload(screens)
}
