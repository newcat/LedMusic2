<template>
    <path :d="d"></path>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";

@Component
export default class Connection extends Vue {

    @Prop({ type: Number })
    x1!: number;
    @Prop({ type: Number })
    y1!: number;

    @Prop({ type: Number })
    x2!: number;
    @Prop({ type: Number })
    y2!: number;

    @Prop({ type: Object })
    data!: any;

    get d() {
        const dx = 0.3 * Math.abs(this.x1 - this.x2);
        return `M ${this.x1} ${this.y1}` +
            `C ${this.x1 + dx} ${this.y1}, ${this.x2 - dx} ${this.y2}, ${this.x2} ${this.y2}`;
    }

}
</script>
