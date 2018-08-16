<template>
    <b-container v-if="state">
        <b-row class="mt-4">

            <b-col cols="6">

                <h4>Outputs</h4>

                <b-dropdown text="Add Output" class="mt-3">
                    <b-dropdown-item
                        v-for="(ot, id) in state.OutputTypes"
                        :key="id"
                        @click="addOutput(id)"
                    >{{ ot.Name }}</b-dropdown-item>
                </b-dropdown>

                <b-list-group class="mt-3">
                    <b-list-group-item
                        v-for="(output, id) in state.Outputs"
                        :key="id"
                        href="#"
                    >{{ output.Name }}</b-list-group-item>
                </b-list-group>

            </b-col>

            <b-col cols="6">
                <h4>Output Options</h4>
                
            </b-col>

        </b-row>
    </b-container>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import { IOutputManager } from "@/types/outputs/outputManager";

@Component
export default class OutputManager extends Vue {

    @Prop({ type: Object, default: () => {} })
    state!: IOutputManager;

    selectedOutputId: string = "";

    addOutput(id: string) {
        this.sendCommand("addOutput", id);
    }

}
</script>
