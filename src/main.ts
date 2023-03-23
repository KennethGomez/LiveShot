import {appWindow, availableMonitors} from '@tauri-apps/api/window';
import {ScreenshotLoader} from './screenshots/loader';
import {KeyboardEvents} from './keyboard/events';
import {ScreenshotWrapper} from './screenshots/wrapper';

const NOT_READY_CLASS: string = 'not-ready';
const EXIT_KEY: string = 'Escape';

class LiveShot {
    private readonly _screenshotLoader: ScreenshotLoader;
    private readonly _screenshotWrapper: ScreenshotWrapper;
    private readonly _keyboard: KeyboardEvents;

    public constructor() {
        this._screenshotLoader = new ScreenshotLoader('get_screenshots');
        this._screenshotWrapper = new ScreenshotWrapper(document.body);
        this._keyboard = new KeyboardEvents();
    }

    public async init(): Promise<void> {
        const elements = await this._screenshotLoader.load();

        await this._screenshotWrapper.displayScreenshots(
            elements, await availableMonitors()
        );

        // Remove not ready class from wrapper
        this._screenshotWrapper.element.classList.remove(NOT_READY_CLASS);

        this._keyboard.registerWindowEvent(EXIT_KEY, async () => {
            await appWindow.close();
        });

        this._keyboard.initWindowEvents();

        // TODO: set always on top
    }
}

const app = new LiveShot();

await app.init();