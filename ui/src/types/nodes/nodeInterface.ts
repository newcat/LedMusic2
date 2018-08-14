import { ConnectionType } from "@/types/connections/connectionType";
import { INodeOption } from "@/types/nodes/nodeOption";
import { IReactiveObject } from "@/types/reactiveObject";

export interface INodeInterface extends IReactiveObject {
    Name: string;
    ConnectionType: ConnectionType;
    IsInput: boolean;
    IsConnected: boolean;
    Option: INodeOption;
}
