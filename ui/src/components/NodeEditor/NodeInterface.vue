<template>
    <div :class="['node-interface', typeClass, { '--input': isInput, '--output': !isInput }]">
        <div class="__port" @mouseover="startHover" @mouseout="endHover"></div>
        <span v-if="!state.IsInput || state.IsConnected" class="align-middle">{{ state.Name }}</span>
        <node-option v-else :data="state.Option"></node-option>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop, Inject } from "vue-property-decorator";
import NodeEditor from "./NodeEditor.vue";
import NodeOption from "./NodeOption";

const typeMapping = [ "number", "color", "color-array", "bool" ];

@Component({
    components: {
        "node-option": NodeOption
    }
})
export default class NodeInterface extends Vue {

    @Prop({ type: Boolean, default: false })
    isInput!: boolean;

    @Prop({ type: Object, default: () => {} })
    state!: any;

    @Inject()
    nodeeditor!: NodeEditor;

    get typeClass() {
        return "--iftype-" + typeMapping[this.state.ConnectionType];
    }

    startHover() {
        this.nodeeditor.hoveredOver(this);
    }
    endHover() {
        this.nodeeditor.hoveredOver(undefined);
    }

}
</script>
