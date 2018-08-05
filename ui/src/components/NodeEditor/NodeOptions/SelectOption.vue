<template>
<!--<div>
    <span>{{ data.Name }}</span>
    <b-form-select v-model="selected" :options="data.Options || []"></b-form-select>
</div>-->
<div
    :class="['node-option-select', { '--open': open }]"
    @click="open = !open"
>
    <div class="__selected">{{ selectedName }}</div>
    <div class="__dropdown">
        <div class="item --header">{{ data.Name }}</div>
        <div
            v-for="(value, key) in data.Options"
            :key="key"
            :class="['item', { '--active': data.SelectedId === key }]"
            @click="sendCommand('SelectedId.set', key)"
        >
            {{ data.ItemDisplayPropertyName ? value[data.ItemDisplayPropertyName] : value }}
        </div>
    </div>
</div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";

@Component
export default class SelectOption extends Vue {

    open = false;

    @Prop({ type: Object, default: () => {} })
    data!: any;

    get selectedName() {
        if (this.data.Options[this.data.SelectedId]) {
            const value = this.data.Options[this.data.SelectedId];
            return this.data.ItemDisplayPropertyName ? value[this.data.ItemDisplayPropertyName] : value;
        } else {
            return "";
        }
    }

}
</script>

<style lang="scss" scoped>

</style>
