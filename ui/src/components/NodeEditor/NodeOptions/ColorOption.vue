<template>
    <b-row>
        <b-col>{{ data.Name }}</b-col>
        <b-col><b-form-input type="color" @input="setColor" ></b-form-input></b-col>
    </b-row>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class ColorOption extends Vue {

    @Prop({ type: Object, default: () => {} })
    data!: any;

    get color() {
        if (!this.data || !this.data.Value) {
            return "#000000";
        } else {
            const rgb = atob(this.data.Value);
            return this.rgbToHex(rgb.charCodeAt(0), rgb.charCodeAt(1), rgb.charCodeAt(2));
        }
    }

    setColor(color: string) {
        const b = btoa(String.fromCharCode(...this.hexToRgb(color)));
        this.sendCommand("setValue", b);
    }

    hexToRgb(hex: string) {
        return hex.replace(/^#?([a-f\d])([a-f\d])([a-f\d])$/i, (m: any, r: string, g: string, b: string) =>
            "#" + r + r + g + g + b + b)
            .substring(1).match(/.{2}/g)!
            .map((x: string) => parseInt(x, 16));
    }

    rgbToHex(r: number, g: number, b: number) {
        // @ts-ignore
        return "#" + [r, g, b].map((x) => x.toString(16).padStart(2, "0")).join("");
    }

}
</script>
