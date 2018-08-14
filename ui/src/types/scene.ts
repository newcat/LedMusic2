import { INodeType } from "@/types/nodes/nodeType";
import { INode } from "@/types/nodes/node";
import { IConnection } from "@/types/connections/connection";
import { IReactiveObject } from "@/types/reactiveObject";

export enum TemporaryConnectionState {
    NONE,
    ALLOWED,
    FORBIDDEN
}

export interface IScene extends IReactiveObject {
    Name: string;
    TemporaryConnectionState: TemporaryConnectionState;
    Nodes: Record<string, INode>;
    Connections: Record<string, IConnection>;
    NodeTypes: Record<string, INodeType>;
}
