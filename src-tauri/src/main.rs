#![cfg_attr(not(debug_assertions), windows_subsystem = "windows")]

use tauri::{AppHandle, Manager, Window};

mod screenshot;

#[tauri::command]
fn show_window(window: Window) {
    window.show().unwrap()
}

fn main() {
    tauri::Builder::default()
        .invoke_handler(tauri::generate_handler![show_window])
        .build(tauri::generate_context!())
        .expect("error while running tauri application")
        .run(|app, event| match event {
            tauri::RunEvent::Ready => bootstrap(app),
            _ => {}
        });
}

fn bootstrap(app: &AppHandle) {
    let window = app.get_window("main").unwrap();
    let handle = app.app_handle();

    window.open_devtools();

    std::thread::spawn(move || {
        let screens = screenshot::capture_all(&window);

        println!("Found and captured {} monitor(s)", screens.len());

        handle.emit_all("screenshot-captured", screenshot::get_payload(screens))
    });
}
