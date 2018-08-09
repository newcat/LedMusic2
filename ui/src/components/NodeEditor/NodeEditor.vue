<template>
    <div
        :class="['node-editor', { 'ignore-mouse': !!temporaryConnection }]"
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
                :status="scene.TemporaryConnectionState"
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
import { IScene } from "@/types/scene";

interface ITemporaryConnection {
    startNode?: Node;
    startInterface?: NodeInterface;
    targetNode?: Node;
    targetInterface?: NodeInterface;
    mx?: number;
    my?: number;
}

@Component({
    components: {
        "node": Node,
        "connection": Connection,
        "temp-connection": TempConnection
    }
})
export default class NodeEditor extends Vue {

    @Prop({ type: Object, default: () => {} })
    scene!: IScene;

    nodes: Record<string, Node> = {};
    temporaryConnection: ITemporaryConnection|null = null;
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
            this.temporaryConnection.targetNode = ni.$parent as Node;
            this.temporaryConnection.targetInterface = ni;
            this.sendCommand("checkTemporaryConnection", {
                originInterfaceId: this.getIdOfInterface(this.temporaryConnection.startInterface!),
                targetInterfaceId: this.getIdOfInterface(this.temporaryConnection.targetInterface!)
            });
        } else if (!ni && this.temporaryConnection) {
            this.$set(this.temporaryConnection, "targetNode", undefined);
            this.$set(this.temporaryConnection, "targetInterface", undefined);
            this.sendCommand("checkTemporaryConnection");
        }
    }

    mouseMoveHandler(ev: MouseEvent) {
        if (!this.temporaryConnection) { return; }
        this.temporaryConnection.mx = ev.offsetX;
        this.temporaryConnection.my = ev.offsetY;
    }

    mouseDown(ev: MouseEvent) {
        if (this.hoveringOver) {

            // if this interface is an input and already has a connection
            // to it, remove the connection and make it temporary
            if (this.hoveringOver.state.IsInput && this.hoveringOver.state.IsConnected) {
                // find connection
                const conn = this.connections.find((c: any) => c.outputInterface === this.hoveringOver);
                this.temporaryConnection = {
                    startNode: conn.inputNode,
                    startInterface: conn.inputInterface
                };
                this.sendCommand("deleteConnection", conn.id);
            } else {
                this.temporaryConnection = {
                    startNode: this.hoveringOver.$parent as Node,
                    startInterface: this.hoveringOver
                };
            }

            this.$set(this.temporaryConnection, "mx", ev.x);
            this.$set(this.temporaryConnection, "my", ev.y);

        }
    }

    mouseUp(ev: MouseEvent) {
        const tc = this.temporaryConnection;
        if (tc && this.hoveringOver) {
            this.sendCommand("addConnection", {
                originInterfaceId: this.getIdOfInterface(tc.startInterface!),
                targetInterfaceId: this.getIdOfInterface(tc.targetInterface!)
            });
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

    getIdOfInterface(intf: NodeInterface) {
        return intf.rname.split(".")[1];
    }

}
</script>
