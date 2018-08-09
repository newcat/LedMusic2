<template>
    <div
        class="node" 
        :style="styles"
    >

        <div
            class="__title"
            @mousedown.prevent="startDrag"
        >
            {{ nodedata.Name }}
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
import { INode } from "@/types/nodes/node";

@Component({
    components: {
        "node-option": NodeOption,
        "node-interface": NodeInterface
    }
})
export default class Node extends Vue {

    @Prop({ type: Object })
    nodedata!: INode;

    @Prop({ type: String })
    id!: string;

    selected = false;
    dragging = false;
    top = 30;
    left = 30;
    width = 200;

    mounted() {
        (this.$parent as any).registerNode(this.id, this);
    }

    beforeDestroy() {
        (this.$parent as any).unregisterNode(this.id);
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
