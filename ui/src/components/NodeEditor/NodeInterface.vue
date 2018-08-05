<template>
    <div :class="['node-interface', { '--input': isInput, '--output': !isInput }]">
        <span class="align-middle">{{ state.Name }}</span>
    </div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";

@Component
export default class NodeInterface extends Vue {

    @Prop({ type: Boolean, default: false })
    isInput!: boolean;

    @Prop({ type: Object, default: () => {} })
    state!: any;

}
</script>

<style lang="scss">
@mixin port() {
    content: "";
    position: absolute;
    width: 10px;
    height: 10px;
    top: 40%;
    background: yellow;
    border-radius: 5px;
}

.node-interface {
    padding: 0.25em 0;
    position: relative;

    &.--input {
        text-align: left;
        padding-left: 0.5em;

        &:before {
            @include port();
            left: -1.1em;
        }

    }
    &.--output {
        text-align: right;
        padding-right: 0.5em;

        &:before {
            @include port();
            right: -1.1em;
        }

    }

}
</style>
