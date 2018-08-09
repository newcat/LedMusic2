export enum NodeCategory {
    INPUT,
    OUTPUT,
    COLOR,
    CONVERTER,
    GENERATOR
}

export interface INodeType {
    Name: string;
    Category: NodeCategory;
}
