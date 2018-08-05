<template>
    <div class="node-editor">
        <node
            v-for="(value, key) in scene.Nodes"
            :key="key"
            :rname="'Nodes.' + key"
            :nodedata="value">
        </node>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import Node from "./Node.vue";

@Component({
    components: {
        "node": Node
    }
})
export default class NodeEditor extends Vue {

    @Prop({ type: Object, default: () => {} })
    scene!: object;

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
</style>
