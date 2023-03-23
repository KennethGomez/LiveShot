import {appWindow} from '@tauri-apps/api/window';
import {ScreenshotLoader} from './screenshots/loader';
import {KeyboardEvents} from './keyboard/events';

const NOT_READY_CLASS: string = 'not-ready';
const EXIT_KEY: string = 'Escape';

class LiveShot {
    private readonly _screenshots: ScreenshotLoader;
    private readonly _keyboard: KeyboardEvents;

    public constructor() {
        this._screenshots = new ScreenshotLoader('get_screenshots');
        this._keyboard = new KeyboardEvents();
    }

    public async init(): Promise<void> {
        const elements = await this._screenshots.load();

        document.body.classList.remove(NOT_READY_CLASS);
        document.body.append(...elements);

        this._keyboard.registerWindowEvent(EXIT_KEY, async () => {
            await appWindow.close();
        });

        this._keyboard.initWindowEvents();
    }
}

const app = new LiveShot();

await app.init();