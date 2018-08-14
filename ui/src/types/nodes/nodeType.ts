import { IReactiveObject } from "@/types/reactiveObject";

export enum NodeCategory {
    INPUT,
    OUTPUT,
    COLOR,
    CONVERTER,
    GENERATOR
}

export interface INodeType extends IReactiveObject {
    Name: string;
    Category: NodeCategory;
}
