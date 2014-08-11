// WARNING
// This file has been generated automatically by Xamarin Studio to
// mirror C# types. Changes in this file made by drag-connecting
// from the UI designer will be synchronized back to C#, but
// more complex manual changes may not transfer correctly.


#import <Foundation/Foundation.h>
#import <AppKit/AppKit.h>


@interface MainWindowController : NSWindowController {
	NSButton *_btnExecute;
	NSScrollView *_txtBox;
    
	NSTextField *_txtHigh;
	NSTextField *_txtLow;
    NSTextView *_txtView;
}

@property (nonatomic, retain) IBOutlet NSButton *btnExecute;

@property (nonatomic, retain) IBOutlet NSScrollView *txtBox;

@property (nonatomic, retain) IBOutlet NSTextField *txtHigh;

@property (nonatomic, retain) IBOutlet NSTextField *txtLow;

@property (assign) IBOutlet NSTextView *txtView;


- (IBAction)btn_execute:(id)sender;

@end
