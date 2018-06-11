/*
 * VRVIUMPMoviePlayerController.h
 *
 * Copyright (c) 2018 VRVIU
 * Copyright (c) 2018 Felix
 */

#import "VRVIUMediaPlayback.h"
#import <MediaPlayer/MediaPlayer.h>

@interface VRVIUMPMoviePlayerController : MPMoviePlayerController <VRVIUMediaPlayback>

- (id)initWithContentURL:(NSURL *)aUrl;
- (id)initWithContentURLString:(NSString *)aUrl;

@end
