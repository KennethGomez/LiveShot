use tauri::{AppHandle, Manager};

use crate::capture_screenshots;

pub fn open_capture_window(app: &AppHandle) {
    let screenshots = capture_screenshots();

    // TODO: create window if doesn't exist
    let window = app.get_window("main").unwrap();

    window.emit("screenshots", screenshots).unwrap();
    window.show().unwrap();
    window.set_always_on_top(true).unwrap();
}
