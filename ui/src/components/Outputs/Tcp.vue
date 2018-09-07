<template>
    <b-input type="number" v-model="state.Port"></b-input>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import { ITcpOutput } from "@/types/outputs/tcpOutput";

@Component
export default class Tcp extends Vue {

    @Prop({ type: Object, default: () => {} })
    state!: ITcpOutput;

}
</script>
