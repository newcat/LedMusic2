<template>
    <div class="node-editor">
        <svg class="connections-container">
            <connection
                v-for="c in connections"
                :key="c.id"
                :data="c"
            ></connection>
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
import { Component, Vue, Prop } from "vue-property-decorator";
import * as _ from "lodash";
import Node from "./Node.vue";
import Connection from "./Connection.vue";

@Component({
    components: {
        "node": Node,
        "connection": Connection
    }
})
export default class NodeEditor extends Vue {

    @Prop({ type: Object, default: () => {} })
    scene!: any;

    nodes: Record<string, any> = {};

    registerNode(id: string, node: any) {
        this.$set(this.nodes, id, node);
    }

    unregisterNode(id: string) {
        this.$delete(this.nodes, id);
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
