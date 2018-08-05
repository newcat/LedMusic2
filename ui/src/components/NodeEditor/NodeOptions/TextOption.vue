<template>
    <div>{{ data.Name }}</div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class ColorOption extends Vue {

    @Prop({ type: Object, default: () => {} })
    data!: any;

}
</script>