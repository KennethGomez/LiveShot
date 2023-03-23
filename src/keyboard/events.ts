import {register, ShortcutHandler} from '@tauri-apps/api/globalShortcut';

type WindowShortcutHandler = (event: KeyboardEvent) => void;

interface WindowKeyboardEventData {
    key: string;
    handler: WindowShortcutHandler;
}

export class KeyboardEvents {
    private readonly _windowEvents: WindowKeyboardEventData[];

    public constructor() {
        this._windowEvents = [];
    }

    public initWindowEvents() {
        document.addEventListener('keydown', (e) => {
            for (const event of this._windowEvents) {
                if (e.key === event.key) {
                    event.handler(e);
                }
            }
        });
    }

    public registerWindowEvent(key: string, handler: WindowShortcutHandler): void {
        this._windowEvents.push({
            key,
            handler
        } as WindowKeyboardEventData);
    }

    public async registerGlobalEvent(key: string, handler: ShortcutHandler): Promise<void> {
        await register(key, handler);
    }
}