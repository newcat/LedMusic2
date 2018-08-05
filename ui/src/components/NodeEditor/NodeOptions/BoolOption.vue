<template>
    <b-checkbox>{{ data.Name }}</b-checkbox>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class BoolOption extends Vue {

    @Prop({ type: Object, default: () => {} })
    data!: any;

}
</script>

