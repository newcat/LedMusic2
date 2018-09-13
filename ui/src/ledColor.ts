export function toHex(b64: string): string {
    const rgb = atob(b64);
    return rgbToHex(rgb.charCodeAt(0), rgb.charCodeAt(1), rgb.charCodeAt(2));
}

export function fromHex(hex: string): string {
    return btoa(String.fromCharCode(...hexToRgb(hex)));
}

function hexToRgb(hex: string): number[] {
    return hex.replace(/^#?([a-f\d])([a-f\d])([a-f\d])$/i, (m: any, r: string, g: string, b: string) =>
        "#" + r + r + g + g + b + b)
        .substring(1).match(/.{2}/g)!
        .map((x: string) => parseInt(x, 16));
}

function rgbToHex(r: number, g: number, b: number) {
    // @ts-ignore
    return "#" + [r, g, b].map((x) => x.toString(16).padStart(2, "0")).join("");
}
