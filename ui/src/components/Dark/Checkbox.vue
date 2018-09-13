<template>
    <b-checkbox
        :value="value"
        @input="$emit('input', $event)"
    ><slot></slot></b-checkbox>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class DarkCheckbox extends Vue {

    @Prop({ type: Boolean, default: false })
    value!: boolean;

}
</script>

