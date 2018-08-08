<template>
    <connection-view :x1="d.x1" :y1="d.y1" :x2="d.x2" :y2="d.y2"></connection-view>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import ConnectionView from "./ConnectionView.vue";

@Component({
    components: {
        "connection-view": ConnectionView
    }
})
export default class ConnectionWrapper extends Vue {

    @Prop({ type: Object })
    data!: any;

    get d() {
        const inode = this.data.inputNode;
        const onode = this.data.outputNode;
        const ii = this.data.inputInterface;
        const oi = this.data.outputInterface;
        if (inode && onode) {
            const x1 = inode.left + inode.$el.clientWidth;
            const y1 = inode.top + ii.$el.offsetTop + ii.$el.clientHeight / 2;
            const x2 = onode.left;
            const y2 = onode.top + oi.$el.offsetTop + oi.$el.clientHeight / 2;
            return { x1, y1, x2, y2 };
        } else {
            return { x1: 0, y1: 0, x2: 0, y2: 0 };
        }
    }

}
</script>
