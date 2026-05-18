export interface CommentModel {
    id: string;
    content: string;
    authorName: string;        // Backend field: AuthorName
    userId: string;
    createdAt: string;
}
export interface AddCommentRequest { content: string; }