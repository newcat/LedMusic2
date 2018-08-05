<template>
    <div
        class="node" 
        :style="styles"
    >

        <div
            class="__title"
            @mousedown.prevent="startDrag"
        >
            Node
        </div>

        <div class="__content">
            <!-- Outputs -->
            <node-interface
                v-for="(output, key) in (nodedata.Outputs)"
                :key="key"
                :rname="'Outputs.' + key"
                :state="output"
            ></node-interface>

            <!-- Options -->
            <node-option
                v-for="(option, key) in (nodedata.Options)"
                :key="key"
                :rname="'Options.' + key"
                :data="option"
            ></node-option>

            <!-- Inputs -->
            <node-interface
                v-for="(input, key) in (nodedata.Inputs)"
                :key="key"
                :rname="'Inputs.' + key"
                :state="input"
                is-input
            ></node-interface>

        </div>

    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import NodeOption from "./NodeOption";
import NodeInterface from "./NodeInterface.vue";

@Component({
    components: {
        "node-option": NodeOption,
        "node-interface": NodeInterface
    }
})
export default class Node extends Vue {

    @Prop({ type: Object })
    nodedata: any;

    selected = false;
    dragging = false;
    top = 30;
    left = 30;
    width = 200;

    mounted() {
    }

    get styles() {
        return {
            top: `${this.top}px`,
            left: `${this.left}px`,
            width: `${this.width}px`,
        };
    }

    startDrag() {
        this.dragging = true;
        document.addEventListener("mousemove", this.handleMove);
        document.addEventListener("mouseup", this.stopDrag);
    }

    stopDrag() {
        this.dragging = false;
        document.removeEventListener("mousemove", this.handleMove);
        document.removeEventListener("mouseup", this.stopDrag);
    }

    handleMove(ev: MouseEvent) {
        if (this.dragging) {
            this.left += ev.movementX;
            this.top += ev.movementY;
        }
    }

}
</script>

<style lang="scss">
.node {
    max-width: 20rem;
    font-size: 0.8rem;
    background: #3f3f3fcc;
    color: white;
    border-radius: 4px;
    position: relative;
    filter: drop-shadow(0 0 3px #000000cc);

    .__title {
        background: black;
        color: white;
        padding: 0.4em 0.75em;
        border-radius: 4px 4px 0 0;
    }

    .__content {
        padding: 0.75em;

        & > div {
            margin: 0.5em 0;
        }

    }

}
</style>
