import Vue from "vue";

export default function apply(state: Record<string, any>, obj: Record<string, any>) {
    Object.keys(state).forEach((k) => {
        const v = state[k];
        if (k === "__Type") {
            // obj.__Type = v;
        } else if (typeof(v) === "string" && v === "__Deleted") {
            Vue.delete(obj, k);
            delete obj[k];
        } else if (v === null) {
            Vue.set(obj, k, null);
        } else if (v.__Type) {
            if (v.__IsPrimitive) {
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
