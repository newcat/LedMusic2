export enum NodeOptionType {
    NUMBER,
    COLOR,
    BOOL,
    SELECT,
    PREVIEW,
    TEXT,
    CUSTOM
}

export interface INodeOption {
    Name: string;
    Type: NodeOptionType;
}
