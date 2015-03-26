﻿using System;
using System.Runtime.InteropServices;

namespace LibuvSharp
{
	public abstract class CallbackHandle : Handle
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		protected delegate void uv_handle_cb(IntPtr handle);

		protected static uv_handle_cb uv_callback;

		static CallbackHandle()
		{
			uv_callback = uv_handle;
		}

		public CallbackHandle(Loop loop, HandleType handleType)
			: base(loop, handleType)
		{
		}

		static void uv_handle(IntPtr handle)
		{
			FromIntPtr<CallbackHandle>(handle).OnCallback();
		}

		public event Action Callback;

		protected void OnCallback()
		{
			if (Callback != null) {
				Callback();
			}
		}

		protected void Invoke(Func<IntPtr, int> function)
		{
			CheckDisposed();

			int r = function(NativeHandle);
			Ensure.Success(r);
		}
	}

	public abstract class StartableCallbackHandle : CallbackHandle
	{
		public StartableCallbackHandle(Loop loop, HandleType handleType)
			: base(loop, handleType)
		{
		}

		public StartableCallbackHandle(Loop loop, HandleType handleType, Func<IntPtr, IntPtr, int> constructor)
			: this(loop, handleType)
		{
			int r = constructor(loop.NativeHandle, NativeHandle);
			Ensure.Success(r);
		}

		public abstract void Start();
		public abstract void Stop();

		protected void Invoke(Func<IntPtr, uv_handle_cb, int> function)
		{
			CheckDisposed();

			int r = function(NativeHandle, uv_callback);
			Ensure.Success(r);
		}
	}
}

