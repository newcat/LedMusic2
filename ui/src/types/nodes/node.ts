import { INodeInterface } from "@/types/nodes/nodeInterface";
import { INodeOption } from "@/types/nodes/nodeOption";
import { IReactiveObject } from "@/types/reactiveObject";

export interface INode extends IReactiveObject {
    Name: string;
    Inputs: Record<string, INodeInterface>;
    Outputs: Record<string, INodeInterface>;
    Options: Record<string, INodeOption>;
}
