<template>
    <b-row>
        <b-col>{{ data.Name }}</b-col>
        <!--<b-col><b-form-input type="color" @input="setColor" ></b-form-input></b-col>-->
        <b-col>
            <d-color-picker
                :value="color"
                @input="setColor"
            ></d-color-picker>
        </b-col>
    </b-row>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import ColorPicker from "@/components/Dark/ColorPicker.vue";
import { fromHex, toHex } from "@/ledColor";

@Component({
    components: {
        "d-color-picker": ColorPicker
    }
})
export default class ColorOption extends Vue {

    @Prop({ type: Object, default: () => {} })
    data!: any;

    get color() {
        console.log(this.data);
        if (!this.data || !this.data.Value) {
            return "#000000";
        } else {
            return toHex(this.data.Value);
        }
    }

    setColor(color: string) {
        this.sendCommand("setValue", fromHex(color));
    }

}
</script>
