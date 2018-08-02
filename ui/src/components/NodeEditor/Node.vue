<template>
<div>
    
    <h3>NodeTest</h3>

    <!-- Outputs -->
    <div
        v-for="(output, key) in (nodedata.outputs || {})"
        :key="key"
    >{{ output.name }}</div>

    <!-- Options -->
    <node-option
        v-for="(option, key) in (nodedata.options || {})"
        :key="key"
        :options="option"
    ></node-option>

    <!-- Inputs -->
    <div
        v-for="(input, key) in (nodedata.inputs || {})"
        :key="key"
    >{{ input.name }}</div>

    <hr>

</div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import NodeOption from "./NodeOption";

@Component({
    components: {
        "node-option": NodeOption
    }
})
export default class Node extends Vue {

    @Prop({ type: Object })
    nodedata: any;

}
</script>
