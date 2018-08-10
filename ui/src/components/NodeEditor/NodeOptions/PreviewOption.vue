<template>
    <div class="node-option-preview" :style="{ background: gradient }"></div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";

@Component
export default class PreviewOption extends Vue {

    @Prop({ type: Object, default: () => {} })
    data!: any;

    get gradient() {

        if (!this.data || !this.data.Value) {
            return "black";
        }

        const colors: string[] = this.data.Value
            .split(",")
            .map((c: string) => atob(c))
            .map((c: string) => `rgb(${c.charCodeAt(0)}, ${c.charCodeAt(1)}, ${c.charCodeAt(2)})`);

        if (colors.length === 1) {
            colors.push(colors[0]);
        }

        return `linear-gradient(90deg, ${colors.join(",")})`;
    }

}
</script>


<style scoped>
.node-option-preview {
    height: 1.5rem;
    background-color: black;
}
</style>
