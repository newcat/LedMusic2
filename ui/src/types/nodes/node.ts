import { INodeInterface } from "@/types/nodes/nodeInterface";
import { INodeOption } from "@/types/nodes/nodeOption";

export interface INode {
    Name: string;
    Inputs: Record<string, INodeInterface>;
    Outputs: Record<string, INodeInterface>;
    Options: Record<string, INodeOption>;
}
