<template>
<div
    :class="['dark-select', { '--open': open }]"
    @click="open = !open"
>
    <div class="__selected">
        <div class="__text">{{ selectedName }}</div>
        <div class="__icon"><i class="material-icons md-18">keyboard_arrow_down</i></div>
    </div>
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
