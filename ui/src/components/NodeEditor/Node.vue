<template>
    <div
        :class="['node', { '--selected': selected }]" 
        :style="styles"
    >

        <div
            class="__title"
            @mousedown.prevent.stop="startDrag"
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
import NodeEditor from "./NodeEditor.vue";
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

    @Prop({ type: Boolean, default: false })
    selected!: boolean;

    dragging = false;
    top = 30;
    left = 30;
    width = 200;

    get parent() {
        return this.$parent as NodeEditor;
    }

    mounted() {
        this.parent.registerNode(this.id, this);
        if (this.nodedata.VisualState) {
            const vs = JSON.parse(this.nodedata.VisualState);
            this.top = vs.top;
            this.left = vs.left;
            this.width = vs.width;
        }
    }

    beforeDestroy() {
        this.parent.unregisterNode(this.id);
    }

    get styles() {
        return {
            top: `${this.top + this.parent.globalTop}px`,
            left: `${this.left + this.parent.globalLeft}px`,
            width: `${this.width}px`,
        };
    }

    startDrag() {
        this.dragging = true;
        document.addEventListener("mousemove", this.handleMove);
        document.addEventListener("mouseup", this.stopDrag);
        this.select();
    }

    select() {
        this.$emit("select", this);
    }

    stopDrag() {
        this.dragging = false;
        document.removeEventListener("mousemove", this.handleMove);
        document.removeEventListener("mouseup", this.stopDrag);
        this.sendVisualState();
    }

    sendVisualState() {
        this.sendCommand("setVisualState", JSON.stringify({
            top: this.top,
            left: this.left,
            width: this.width
        }));
    }

    handleMove(ev: MouseEvent) {
        if (this.dragging) {
            this.left += ev.movementX;
            this.top += ev.movementY;
        }
    }

}
</script>
