<template>
    <path :d="d"></path>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";

@Component
export default class Connection extends Vue {

    @Prop({ type: Object })
    data!: any;

    get d() {
        const inode = this.data.inputNode;
        const onode = this.data.outputNode;
        const ii = this.data.inputInterface;
        const oi = this.data.outputInterface;
        if (inode && onode) {
            const x1 = inode.left + inode.$el.clientWidth;
            const y1 = inode.top + ii.$el.offsetTop + ii.$el.clientHeight / 2 + 2;
            const x2 = onode.left;
            const y2 = onode.top + oi.$el.offsetTop + oi.$el.clientHeight / 2 + 2;
            const dx = 0.3 * Math.abs(x1 - x2);
            return `M ${x1} ${y1} C ${x1 + dx} ${y1}, ${x2 - dx} ${y2}, ${x2} ${y2}`;
        } else {
            return "";
        }
    }

}
</script>
