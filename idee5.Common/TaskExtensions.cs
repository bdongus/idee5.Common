﻿using System;
using System.Threading.Tasks;

namespace idee5.Common {
    /// <summary>
    /// Extension methods for System.Threading.Tasks.Task
    /// </summary>
    /// // Taken form https://github.com/brminnick/AsyncAwaitBestPractices/
    public static class TaskExtensions {
        /// <summary>
        /// Safely execute the <see cref="Task"/> without waiting for it to complete before moving to the next line of code;
        /// commonly known as "Fire And Forget".
        /// Inspired by John Thiriet's blog post, "Removing Async Void": https://johnthiriet.com/removing-async-void/.
        /// </summary>
        /// <param name="task">Task.</param>
        /// <param name="continueOnCapturedContext">If set to <c>true</c> continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c> continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
        /// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
        /// <exception cref="ArgumentNullException"><paramref name="task"/> is <c>null</c>.</exception>
        public static async void SafeFireAndForget(this Task task, bool continueOnCapturedContext = true, Action<Exception> onException = null) {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            try {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex) when (onException != null) {
                onException(ex);
            }
        }
    }
}
