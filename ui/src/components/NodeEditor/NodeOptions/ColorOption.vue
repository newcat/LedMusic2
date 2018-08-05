<template>
    <b-row>
        <b-col>{{ data.Name }}</b-col>
        <b-col><b-form-input type="color"></b-form-input></b-col>
    </b-row>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class ColorOption extends Vue {

    @Prop({ type: Object, default: () => {} })
    data!: any;

}
</script>
