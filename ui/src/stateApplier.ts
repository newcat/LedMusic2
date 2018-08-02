import Vue from "vue";

const primitiveTypes = [
    "System.Int32", "System.Boolean", "System.String", "LedMusic2.Nodes.NodeCategory"
];

export default function apply(state: Record<string, any>, obj: Record<string, any>) {
    Object.keys(state).forEach((k) => {
        const v = state[k];
        if (k === "__Type") {
            // obj.__Type = v;
        } else if (v === null) {
            Vue.set(obj, k, null);
        } else if (v.__Type) {
            if (primitiveTypes.indexOf(v.__Type) > -1) {
                Vue.set(obj, k, v.Value);
            } else {
                const r = {};
                apply(v, r);
                Vue.set(obj, k, r);
            }
        } else if (typeof(obj[k]) === "object") {
            apply(v, obj[k]);
        } else if (typeof(obj[k]) === "undefined") {
            throw new Error(`Non existing property ${k} without type.`);
        } else {
            Vue.set(obj, k, v.Value);
        }
    });
}
