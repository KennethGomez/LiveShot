import {appWindow, Monitor, PhysicalPosition, PhysicalSize} from '@tauri-apps/api/window';

export class ScreenshotWrapper {
    public constructor(
        private readonly _element: HTMLElement,
    ) {
    }

    public async displayScreenshots(screenshots: Map<string, HTMLImageElement>, monitors: Monitor[]): Promise<void> {
        const xMin = Math.min(...monitors.map(m => m.position.x));
        const yMin = Math.min(...monitors.map(m => m.position.y));
        const xMax = Math.max(...monitors.map(m => m.position.x + m.size.width));
        const yMax = Math.max(...monitors.map(m => m.position.y + m.size.height));

        const elements = [];

        let currentX = 0;

        for (const monitor of monitors.sort((a, b) => a.position.x - b.position.x)) {
            if (monitor.name === null) continue;

            const screenshot = screenshots.get(monitor.name);

            // TODO: Handle non existing screenshot for that monitor
            if (screenshot === undefined) continue;

            screenshot.style.left = currentX + 'px';
            screenshot.style.top = monitor.position.y + 'px';

            currentX += monitor.size.width;

            elements.push(screenshot);
        }

        this._element.append(...elements);

        await appWindow.setPosition(new PhysicalPosition(xMin, yMin));
        await appWindow.setSize(new PhysicalSize(xMax - xMin, yMax - yMin));
    }

    public get element(): HTMLElement {
        return this._element;
    }
}