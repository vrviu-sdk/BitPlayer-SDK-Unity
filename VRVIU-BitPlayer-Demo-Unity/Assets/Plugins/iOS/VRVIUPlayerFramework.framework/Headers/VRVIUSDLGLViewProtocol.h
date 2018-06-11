/*
 * VRVIUSDLGLViewProtocol.h
 *
 * Copyright (c) 2018 VRVIU
 * Copyright (c) 2017 felix
 *
 */

#ifndef VRVIUSDLGLViewProtocol_h
#define VRVIUSDLGLViewProtocol_h

#import <UIKit/UIKit.h>

typedef struct VRVIUOverlay VRVIUOverlay;
struct VRVIUOverlay {
    int w;
    int h;
    UInt32 format;
    int planes;
    UInt16 *pitches;
    UInt8 **pixels;
    int sar_num;
    int sar_den;
    CVPixelBufferRef pixel_buffer;
    // add by felix
    //int is_enable_mapping;
    // add end 2018.4.11
};

@protocol VRVIUSDLGLViewProtocol <NSObject>
- (UIImage*) snapshot;
@property(nonatomic, readonly) CGFloat  fps;
@property(nonatomic)        CGFloat  scaleFactor;
@property(nonatomic)        BOOL  isThirdGLView;
- (void) display_pixels: (VRVIUOverlay *) overlay;
@end

#endif /* VRVIUSDLGLViewProtocol_h */
