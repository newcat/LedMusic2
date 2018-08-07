<template>
    <div
        class="node-editor"
        @mousemove="mouseMoveHandler"
        @mousedown="mouseDown"
        @mouseup="mouseUp"
    >
        <svg class="connections-container">
            <connection
                v-for="c in connections"
                :key="c.id"
                :data="c"
            ></connection>
            <temp-connection
                v-if="temporaryConnection"
                :data="temporaryConnection"
            ></temp-connection>
        </svg>
        <node
            v-for="(value, key) in scene.Nodes"
            :key="key"
            :id="key"
            :rname="'Nodes.' + key"
            :nodedata="value">
        </node>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop, Provide } from "vue-property-decorator";
import * as _ from "lodash";
import Node from "./Node.vue";
import Connection from "./ConnectionWrapper.vue";
import TempConnection from "./TemporaryConnection.vue";
import NodeInterface from "./NodeInterface.vue";

@Component({
    components: {
        "node": Node,
        "connection": Connection,
        "temp-connection": TempConnection
    }
})
export default class NodeEditor extends Vue {

    @Prop({ type: Object, default: () => {} })
    scene!: any;

    nodes: Record<string, any> = {};
    temporaryConnection: any = null;
    hoveringOver?: NodeInterface|null = null;

    @Provide("nodeeditor")
    nodeeditor: NodeEditor = this;

    registerNode(id: string, node: any) {
        this.$set(this.nodes, id, node);
    }

    unregisterNode(id: string) {
        this.$delete(this.nodes, id);
    }

    hoveredOver(ni: NodeInterface|undefined) {
        this.hoveringOver = ni;
        if (ni && this.temporaryConnection && this.temporaryConnection.startInterface !== ni) {
            this.temporaryConnection.targetNode = ni.$parent;
            this.temporaryConnection.targetInterface = ni;
        } else if (!ni && this.temporaryConnection) {
            this.$set(this.temporaryConnection, "targetNode", undefined);
            this.$set(this.temporaryConnection, "targetInterface", undefined);
        }
    }

    mouseMoveHandler(ev: MouseEvent) {
        if (!this.temporaryConnection) { return; }
        this.temporaryConnection.mx = ev.offsetX;
        this.temporaryConnection.my = ev.offsetY;
    }

    mouseDown(ev: MouseEvent) {
        if (this.hoveringOver) {
            this.temporaryConnection = {
                startNode: this.hoveringOver.$parent,
                startInterface: this.hoveringOver,
                mx: ev.x,
                my: ev.y
            };
        }
    }

    mouseUp(ev: MouseEvent) {
        const tc = this.temporaryConnection;
        if (this.hoveringOver && this.hoveringOver !== (tc.outputNode || tc.inputNode)) {
            // TODO: Create
        }
        this.temporaryConnection = null;
    }

    get connections() {
        const arr: any = [];
        for (const pair of _.toPairs(this.scene.Connections)) {
            const [id, value] = pair as [string, any];
            const inputNode = this.nodes[value.InputNodeId];
            const outputNode = this.nodes[value.OutputNodeId];
            const inputInterface = this.getInterface(inputNode, value.InputNodeId, value.InputInterfaceId, false);
            const outputInterface = this.getInterface(outputNode, value.OutputNodeId, value.OutputInterfaceId, true);
            if (inputNode && outputNode && inputInterface && outputInterface) {
                arr.push({ id, inputNode, outputNode, inputInterface, outputInterface });
            }
        }
        return arr;
    }

    getInterface(node: any, nodeId: string, interfaceId: string, isInput: boolean) {
        const io = isInput ? "Inputs" : "Outputs";
        if (node) { return node.$children.find((i: any) => i.state === this.scene.Nodes[nodeId][io][interfaceId]); }
    }

}
</script>


<style lang="scss" scoped>
@import "../../styles/imports";

.node-editor {
    width: 100%;
    height: 100%;
    position: relative;
    overflow: hidden;
    @include backgroundpattern();

    & > * {
        user-select: none;
    }

}

.connections-container {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    pointer-events: none;

    & > path {
        stroke: white;
        stroke-width: 2px;
        fill: none;
    }

}
</style>
