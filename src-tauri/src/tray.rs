use tauri::{
    AppHandle, CustomMenuItem, SystemTray, SystemTrayEvent, SystemTrayMenu, SystemTrayMenuItem,
};

pub fn get_liveshot_system_tray() -> SystemTray {
    let tray_menu = SystemTrayMenu::new()
        .add_item(CustomMenuItem::new(
            "capture".to_string(),
            "Take a screenshot",
        ))
        .add_native_item(SystemTrayMenuItem::Separator)
        .add_item(CustomMenuItem::new("exit".to_string(), "Exit"));

    SystemTray::new().with_menu(tray_menu)
}

pub fn handle_system_tray_event(app: &AppHandle, event: SystemTrayEvent) {
    match event {
        SystemTrayEvent::MenuItemClick { id, .. } => handle_system_tray_menu_click(app, id),
        SystemTrayEvent::LeftClick { .. } | SystemTrayEvent::DoubleClick { .. } => {
            // TODO: capture
        }
        _ => {}
    }
}

fn handle_system_tray_menu_click(app: &AppHandle, id: String) {
    // TODO: capture

    match id.as_str() {
        "exit" => app.exit(0),
        &_ => {}
    }
}
