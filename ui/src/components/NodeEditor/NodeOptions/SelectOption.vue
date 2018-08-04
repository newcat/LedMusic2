<template>
<div>
    <p>{{ data.Name }}</p>
    <b-form-select v-model="selected" :options="data.Options || []"></b-form-select>
</div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";

@Component
export default class SelectOption extends Vue {

    selected = "";

    @Prop({ type: Object, default: () => {} })
    data!: object;

}
</script>
