/****** Script for SelectTopNRows command from SSMS  ******/
SELECT threadSnippet.TopLevelCommentId 'CommentId',
	   video.Id as 'VideoId',	
       snippet.ChannelId as 'ChannelId',
	   snippet.Localized_Title as 'ChannelTitle',
	   snippet.Localized_Description as 'Description',
	   snippet.PublishedAtDateTimeOffset as 'PublishedDateTimeOffset'

FROM [Youtube_Video] video
Join [Youtube_VideoSnippet] snippet On snippet.Our_Id = video.SnippetId
Join [Youtube_CommentThreadSnippet] threadSnippet On threadSnippet.VideoId = video.Id
Join [Youtube_Comment] comment on comment.Id = threadSnippet.TopLevelCommentId
Join [Youtube_CommentSnippet] commentSnippet on commentSnippet.ParentId = comment.Id
where video.Id = 'mriqfI6b_-Y'
