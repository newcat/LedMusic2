<template>
    <connection-view
        :x1="d.input.x" :y1="d.input.y"
        :x2="d.output.x" :y2="d.output.y"
    ></connection-view>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import ConnectionView from "./ConnectionView.vue";

enum ConnectionStatus {
    ALLOWED,
    LOADING,
    FORBIDDEN
}

@Component({
    components: {
        "connection-view": ConnectionView
    }
})
export default class TemporaryConnection extends Vue {

    @Prop({ type: Object })
    data!: any;

    status: ConnectionStatus = ConnectionStatus.ALLOWED;

    get d() {

        const start = this.getCoords(this.data.startNode, this.data.startInterface);
        const end = this.data.targetNode ?
                this.getCoords(this.data.targetNode, this.data.targetInterface) :
                { x: this.data.mx, y: this.data.my };

        if (this.data.startInterface.state.IsInput) {
            return {
                input: end,
                output: start
            };
        } else {
            return {
                input: start,
                output: end
            };
        }

    }

    getCoords(node: any, intf: any) {
        const x = intf.state.IsInput ? node.left : node.left + node.$el.clientWidth;
        const y = node.top + intf.$el.offsetTop + intf.$el.clientHeight / 2 + 2;
        return { x, y };
    }

}
</script>
