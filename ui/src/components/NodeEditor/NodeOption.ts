import Vue from "vue";

enum NodeOptionType {
    NUMBER,
    COLOR,
    BOOL,
    SELECT,
    PREVIEW,
    TEXT
}

import BoolOption from "./NodeOptions/BoolOption.vue";
import ColorOption from "./NodeOptions/ColorOption.vue";
import NumberOption from "./NodeOptions/NumberOption.vue";
import PreviewOption from "./NodeOptions/PreviewOption.vue";
import SelectOption from "./NodeOptions/SelectOption.vue";
import TextOption from "./NodeOptions/TextOption.vue";
const elements = [
    NumberOption,
    ColorOption,
    BoolOption,
    SelectOption,
    PreviewOption,
    TextOption
];

export default Vue.extend({

    functional: true,

    props: {
        data: {
            type: Object,
            required: true
        }
    },

    render(createElement, context) {
        const type = context.props.data.Type as NodeOptionType;
        return createElement(elements[type], {
            props: context.props,
            staticClass: context.data.staticClass
        });
    }

});