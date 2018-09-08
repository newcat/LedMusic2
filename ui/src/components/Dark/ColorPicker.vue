<template>
    <div
        @click.self="open = true"
        class="dark-color-picker"
        :style="styles"
        v-click-outside="() => { open = false; }"
    >
        <transition name="slide-fade">
            <cp-chrome
                v-show="open"
                :value="value"
                @input="$emit('input', $event.hex)"
                class="color-picker"
            ></cp-chrome>
        </transition>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
// @ts-ignore
import { Chrome } from "vue-color";

@Component({
    components: {
        "cp-chrome": Chrome
    }
})
export default class ColorPicker extends Vue {

    @Prop({ type: String, default: "#000000" })
    value!: string;

    open = false;

    get styles() {
        return {
            "background-color": this.value
        };
    }

}
</script>

<style scoped>
.color-picker {
    position: absolute;
    left: 100%;
    top: 0%;
    z-index: 100;
}
</style>
