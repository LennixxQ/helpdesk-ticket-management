export interface CommentModel {
    id: string;
    content: string;
    postedByUserName: string;
    userId: string;
    ticketId: string;
    createdAt: string;
}
export interface AddCommentRequest { content: string; }