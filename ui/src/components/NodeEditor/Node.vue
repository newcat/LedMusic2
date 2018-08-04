<template>
    <vue-drag-resize>
        <b-card header="Node" style="margin: 1rem; max-width: 20rem;">

            <!-- Outputs -->
            <div
                v-for="(output, key) in (nodedata.Outputs || {})"
                :key="key"
            >{{ output.name }}</div>

            <!-- Options -->
            <node-option
                v-for="(option, key) in (nodedata.Options)"
                :key="key"
                :options="option"
                class="mb-2"
            ></node-option>

            <!-- Inputs -->
            <div
                v-for="(input, key) in (nodedata.Inputs || {})"
                :key="key"
            >{{ input.name }}</div>

        </b-card>
    </vue-drag-resize>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import NodeOption from "./NodeOption";
// @ts-ignore
import VueDragResize from "vue-drag-resize";

@Component({
    components: {
        "node-option": NodeOption,
        "vue-drag-resize": VueDragResize
    }
})
export default class Node extends Vue {

    @Prop({ type: Object })
    nodedata: any;

}
</script>
